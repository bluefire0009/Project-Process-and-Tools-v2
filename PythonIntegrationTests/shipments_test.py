import http.client
import json
import pytest
from datetime import datetime
import time


@pytest.fixture()
def test_body():
    return {
        'id': 999999999,
        'order_id': 999999999,
        'source_id': 33,
        'order_date': 'test_order_date',
        'request_date': 'test_request_date',
        'shipment_date': 'test_shipment_date',
        'shipment_type': 'Test_I',
        'shipment_status': '',
        'notes': 'test_notes',
        'carrier_code': 'DPD',
        'carrier_description': 'Dynamic Parcel Distribution',
        'service_code': 'Fastest',
        'payment_type': 'Manual',
        'transfer_mode': 'Ground',
        'total_package_count': 31,
        'total_package_weight': 594.42,
        'created_at': '',
        'updated_at': '',
        'items': [
            {'item_id': 'P007435', 'amount': 23},
            {'item_id': 'P009557', 'amount': 1}
        ]
    }


@pytest.fixture()
def connection() -> http.client.HTTPConnection:
    return http.client.HTTPConnection('localhost', 3000)


@pytest.fixture()
def headers():
    return {'API_KEY': 'a1b2c3d4e5', 'Content-Type': 'application/json'}


def delete_test_shipment(connection: http.client.HTTPConnection, headers, id=999999999):
    # delete shipment with 'id': 999999999
    connection.request('DELETE', f'/api/v1/shipments/{id}', headers=headers)
    response = connection.getresponse()
    connection.close()
    return response.status


def get_posted_shipment(connection: http.client.HTTPConnection, headers, id: int):
    # this test doesnt run standalone but is part of multiple tests
    connection.request('GET', f"/api/v1/shipments/{id}", headers=headers)

    response = connection.getresponse()
    data = response.read()
    connection.close()

    try:
        json_data = json.loads(data)
    except BaseException:
        return b'null', response.status

    return json.loads(data), response.status


def post_test_shipment(connection: http.client.HTTPConnection, headers, test_body):
    json_body = json.dumps(test_body).encode('utf-8')
    # post shipment
    connection.request('POST', '/api/v1/shipments', headers=headers, body=json_body)
    # get response
    post_response = connection.getresponse()
    # close connection
    post_response.close()

    return test_body, post_response.status


def test_get_all_shipments(connection: http.client.HTTPConnection, headers):
    connection.request('GET', '/api/v1/shipments', headers=headers)

    response = connection.getresponse()
    assert response.status == 200
    data = json.loads(response.read())
    assert isinstance(data, list)
    connection.close()


def test_post_get_shipment(connection: http.client.HTTPConnection, headers, test_body):
    body, response = post_test_shipment(connection, headers, test_body)
    assert response == 201

    # Get shipment
    connection.request('GET', '/api/v1/shipments/999999999', headers=headers)

    response = connection.getresponse()
    data = response.read()
    connection.close()

    shipmentDict = json.loads(data)
    # check if response json has all 19 fields

    assert len(shipmentDict) == 19
    assert shipmentDict['notes'] == 'test_notes'

    # cleanup
    delete_test_shipment(connection, headers)


def test_put_shipment(connection: http.client.HTTPConnection, headers, test_body):
    body, response = post_test_shipment(connection, headers, test_body)
    assert response == 201

    # change the shipment json object before PUT request
    body['source_id'] = 9999
    json_body = json.dumps(body).encode('utf-8')
    connection.request('PUT', '/api/v1/shipments/999999999', headers=headers, body=json_body)

    post_response = connection.getresponse()
    assert post_response.status == 200
    post_response.close()

    # Get shipment
    connection.request('GET', '/api/v1/shipments/999999999', headers=headers)

    response = connection.getresponse()
    data = response.read()
    connection.close()

    shipmentDict = json.loads(data)
    # check if response json has all 19 fields

    assert len(shipmentDict) == 19
    assert shipmentDict['source_id'] == 9999

    delete_test_shipment(connection, headers)


def test_delete_shipment(connection: http.client.HTTPConnection, headers, test_body):
    body, response = post_test_shipment(connection, headers, test_body)
    assert response == 201

    # delete shipment with 'id': 999999999
    delete_test_shipment(connection, headers)

    # Get shipment
    data, response = get_posted_shipment(connection, headers, 999999999)

    assert response == 200


def test_delte_shipment_with_wrong_id(connection: http.client.HTTPConnection, headers):
    # trying to delete something that doesnt exist should return 404 not found
    assert delete_test_shipment(connection, headers, id=99999999999999999999999) == 404


def test_get_with_wrong_id(connection: http.client.HTTPConnection, headers):
    location_dict, status_code = get_posted_shipment(connection, headers, 9999999999999999999999999999999999999)
    # trying to get something that doesnt exist should return 404 not found
    assert status_code == 404


def test_post_with_incorrect_data_in_field(connection: http.client.HTTPConnection, headers, test_body):
    incorrect_body = test_body
    # notes should be a string
    incorrect_body["notes"] = 99
    body, response = post_test_shipment(connection, headers, incorrect_body)

    if (response == 201):
        # if it posts a bad object this line cleans it up
        assert delete_test_shipment(connection, headers) == 200

    # it should return bad request
    assert response == 400


def test_post_with_missing_fields(connection: http.client.HTTPConnection, headers,):
    # all fields missing except for id
    incorrect_body = {'id': 999999999}
    body, response = post_test_shipment(connection, headers, incorrect_body)

    if (response == 201):
        # if it posts a bad object this line cleans it up
        assert delete_test_shipment(connection, headers) == 200

    # it should return bad request
    assert response == 400


def check_time_string_format(connection, headers, date_string):
    # this test will fail: the datimestring being written is in the wrong format
    try:
        # Attempt to parse with the expected format
        # the correct format is taken from the existing objects in shipments.json
        datetime.strptime(date_string, "%Y-%m-%dT%H:%M:%SZ")
        return True
    except BaseException:
        # if it breaks clean up the file
        delete_test_shipment(connection, headers)
        return False


def test_created_at(connection: http.client.HTTPConnection, headers, test_body):
    # created at is empty in the body that gets posted
    body, response = post_test_shipment(connection, headers, test_body)
    assert response == 201

    order_dict, response = get_posted_shipment(connection, headers, body['id'])
    assert response == 200

    assert order_dict["created_at"] != ""

    # assert that the created at is in the right format
    assert check_time_string_format(
        connection, headers, order_dict["created_at"]), f"Datetime fromat incorrect: {order_dict['created_at']}"

    # turn string into datetime obj
    read_date_obj = datetime.strptime(order_dict["created_at"], "%Y-%m-%dT%H:%M:%SZ")
    current_time_obj = datetime.now()

    # make sure time is accurate by minutes
    assert read_date_obj.year == current_time_obj.year
    assert read_date_obj.month == current_time_obj.month
    assert read_date_obj.day == current_time_obj.day
    assert read_date_obj.hour == current_time_obj.hour
    assert read_date_obj.minute == current_time_obj.minute

    assert delete_test_shipment(connection, headers) == 200


def test_updated_at(connection: http.client.HTTPConnection, headers, test_body):
    # created at is empty in the body that gets posted
    body = post_test_shipment(connection, headers, test_body)

    # wait one seconds so the created object will not be the same as the updated
    time.sleep(2)
    # update te location
    body['notes'] = 'changed_notes'
    json_body = json.dumps(body).encode('utf-8')
    connection.request('PUT', f"/api/v1/shipments/{body['id']}", headers=headers, body=json_body)
    connection.close()

    # get the location ad check if updated_at is empty
    shipment_dict, response = get_posted_shipment(connection, headers, body['id'])
    assert response == 200
    assert shipment_dict["updated_at"] != ""

    # assert that the created at is in the right format
    assert check_time_string_format(
        connection, headers, shipment_dict["updated_at"]), f"Datetime fromat incorrect: {shipment_dict['updated_at']}"

    # turn string into datetime obj
    read_date_obj = datetime.strptime(shipment_dict["updated_at"], "%Y-%m-%d %H:%M:%S")
    current_time_obj = datetime.now()

    assert read_date_obj.year == current_time_obj.year
    assert read_date_obj.month == current_time_obj.month
    assert read_date_obj.day == current_time_obj.day
    assert read_date_obj.hour == current_time_obj.hour
    assert read_date_obj.minute == current_time_obj.minute

    # make sure the created_at and updated_at are not the same
    assert shipment_dict["updated_at"] != shipment_dict["created_at"]

    assert delete_test_shipment(connection, headers) == 200

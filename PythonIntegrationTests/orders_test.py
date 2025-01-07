import http.client
import json
import pytest
from datetime import datetime
import time


@pytest.fixture()
def connection() -> http.client.HTTPConnection:
    return http.client.HTTPConnection('localhost', 3000)


@pytest.fixture()
def headers():
    return {'API_KEY': 'a1b2c3d4e5', 'Content-Type': 'application/json'}


def delete_test_order(connection: http.client.HTTPConnection, headers, id=99999):
    # delete location with 'id': 99999
    connection.request('DELETE', '/api/v1/orders/99999', headers=headers)
    response = connection.getresponse()
    connection.close()
    return response.status


def get_posted_order(connection: http.client.HTTPConnection, headers, id: int):
    # this test doesnt run standalone but is part of multiple tests
    connection.request('GET', f"/api/v1/orders/{id}", headers=headers)

    response = connection.getresponse()

    data = response.read()
    connection.close()
    return json.loads(data), response.status


def post_test_order(connection: http.client.HTTPConnection, headers):
    body = {
        'id': 99999,
        'source_id': 33,
        'order_date': '2019-04-03T11:33:15Z',
        'request_date': '2019-04-07T11:33:15Z',
        'reference': 'ORD00001',
        'reference_extra': 'Bedreven arm straffen bureau.',
        'order_status': 'Delivered',
        'notes': 'test_notes',
        'shipping_notes': 'AAAAAAAAAAAAAAa',
        'picking_notes': 'Ademen fijn volgorde scherp aardappel op leren.',
        'warehouse_id': 18,
        'ship_to': None,
        'bill_to': None,
        'shipment_id': 1,
        'total_amount': 9905.13,
        'total_discount': 150.77,
        'total_tax': 372.72,
        'total_surcharge': 77.6,
        'created_at': '2019-04-03T11:33:15Z',
        'updated_at': '2019-04-05T07:33:15Z',
        'items': [
            {
                'item_id': 'P007435',
                'amount': 23
            },
            {
                'item_id': 'P009557',
                'amount': 1
            }
        ]
    }
    json_body = json.dumps(body).encode('utf-8')

    # post location
    connection.request('POST', '/api/v1/orders/', headers=headers, body=json_body)
    # get response
    response = connection.getresponse()
    # assert that it has created
    assert response.status == 201
    # close connection
    response.close()

    return body


def test_get_all_orders(connection: http.client.HTTPConnection, headers):
    connection.request('GET', '/api/v1/orders', headers=headers)

    response = connection.getresponse()
    assert response.status == 200
    data = json.loads(response.read())
    assert isinstance(data, list)
    connection.close()


def test_post_get_orders(connection: http.client.HTTPConnection, headers):
    body = post_test_order(connection, headers)

    # get the body just posted
    connection.request('GET', f"/api/v1/orders/{body['id']}", headers=headers)

    response = connection.getresponse()
    assert response.status == 200

    data = response.read()
    connection.close()

    order_dict = json.loads(data)
    assert len(order_dict) == len(body)
    assert order_dict['notes'] == body['notes']
    assert order_dict['shipping_notes'] == body['shipping_notes']

    assert delete_test_order(connection, headers) == 200


def test_put_order(connection: http.client.HTTPConnection, headers):
    body = post_test_order(connection, headers)

    # adjust order and PUT it
    body['notes'] = 'changed_notes'
    json_body = json.dumps(body).encode('utf-8')
    connection.request('PUT', f"/api/v1/orders/{body['id']}", headers=headers, body=json_body)
    connection.close()

    # GET adjusted order
    connection.request('GET', f"/api/v1/orders/{body['id']}", headers=headers)

    response = connection.getresponse()
    assert response.status == 200

    data = response.read()
    connection.close()

    location_dict = json.loads(data)
    assert len(location_dict) == len(body)
    # loop trough response and original to check if they are the same
    for response_item, original in zip(location_dict, body):
        # skip time created and updated since this is variable
        if body["created_at"] == original or body["updated_at"] == original:
            continue
        assert response_item == original

    assert delete_test_order(connection, headers) == 200


def test_get_order_items(connection: http.client.HTTPConnection, headers):
    body = post_test_order(connection, headers)

    connection.request('GET', f"/api/v1/orders/{body['id']}/items", headers=headers)
    response = connection.getresponse()
    assert response.status == 200

    data = response.read()
    connection.close()
    order_dict = json.loads(data)

    # assert if items are the same
    for original_item, response_item in zip(body['items'], order_dict):
        assert original_item == response_item

    assert delete_test_order(connection, headers) == 200


def test_delete_order(connection: http.client.HTTPConnection, headers):
    # post location
    post_test_order(connection, headers)
    # delte location
    assert delete_test_order(connection, headers) == 200


def test_delte_order_with_wrong_id(connection: http.client.HTTPConnection, headers):
    # trying to delete something that doesnt exist should return 404 not found
    assert delete_test_order(connection, headers, id=99999999999999999999999) == 404


def test_get_order_with_wrong_id(connection: http.client.HTTPConnection, headers):
    location_dict, status_code = get_posted_order(connection, headers, 9999999999999999999999999999999999999)
    # trying to get something that doesnt exist should return 404 not found
    assert status_code == 404


def check_time_string_format(connection, headers, date_string):
    # this test will fail: the datimestring being written is in the wrong format
    try:
        # Attempt to parse with the expected format
        # the correct format is taken from the existing objects in orders.json
        datetime.strptime(date_string, "%Y-%m-%dT%H:%M:%SZ")
        return True
    except BaseException:
        # if it breaks clean up the file
        delete_test_order(connection, headers)
        return False


def test_created_at(connection: http.client.HTTPConnection, headers):
    # created at is empty in the body that gets posted
    body = post_test_order(connection, headers)
    order_dict, response = get_posted_order(connection, headers, body['id'])
    assert response == 200

    assert order_dict["created_at"] != ""

    # assert that the created at is in the right format
    assert check_time_string_format(connection, headers, order_dict["created_at"]), f"Datetime fromat incorrect: {order_dict['created_at']}"

    # turn string into datetime obj
    read_date_obj = datetime.strptime(order_dict["created_at"], "%Y-%m-%dT%H:%M:%SZ")
    current_time_obj = datetime.now()

    # make sure time is accurate by minutes
    assert read_date_obj.year == current_time_obj.year
    assert read_date_obj.month == current_time_obj.month
    assert read_date_obj.day == current_time_obj.day
    assert read_date_obj.hour == current_time_obj.hour
    assert read_date_obj.minute == current_time_obj.minute

    assert delete_test_order(connection, headers) == 200


def test_updated_at(connection: http.client.HTTPConnection, headers):
    # created at is empty in the body that gets posted
    body = post_test_order(connection, headers)

    # wait one seconds so the created object will not be the same as the updated
    time.sleep(2)
    # update te location
    body['notes'] = 'changed_notes'
    json_body = json.dumps(body).encode('utf-8')
    connection.request('PUT', f"/api/v1/orders/{body['id']}", headers=headers, body=json_body)
    connection.close()

    # get the location ad check if updated_at is empty
    order_dict, response = get_posted_order(connection, headers, body['id'])
    assert response == 200
    assert order_dict["updated_at"] != ""

    # assert that the created at is in the right format
    assert check_time_string_format(connection, headers, order_dict["updated_at"]), f"Datetime fromat incorrect: {order_dict['updated_at']}"

    # turn string into datetime obj
    read_date_obj = datetime.strptime(order_dict["updated_at"], "%Y-%m-%d %H:%M:%S")
    current_time_obj = datetime.now()

    assert read_date_obj.year == current_time_obj.year
    assert read_date_obj.month == current_time_obj.month
    assert read_date_obj.day == current_time_obj.day
    assert read_date_obj.hour == current_time_obj.hour
    assert read_date_obj.minute == current_time_obj.minute

    # make sure the created_at and updated_at are not the same
    assert order_dict["updated_at"] != order_dict["created_at"]

    assert delete_test_order(connection, headers) == 200

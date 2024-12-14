import http.client
import json
import pytest

@pytest.fixture
def _DataPytestFixture():
    _connection = http.client.HTTPConnection('localhost', 3000)
    _url = "/api/v2/warehouses"
    _testWarehouse = [{"id": 1000001, "code": "YQZZNL56", "name": "Heemskerk cargo hub", "address": "Karlijndreef 281", "zip": "4002 AS", "city": "Heemskerk", "province": "Friesland", "country": "NL", "contact": {"name": "Fem Keijzer", "phone": "(078) 0013363", "email": "blamore@example.net"}, "created_at": "1983-04-13 04:59:55", "updated_at": "2007-02-08 20:11:00"}, 
            {"id": 1000002, "code": "GIOMNL90", "name": "Petten longterm hub", "address": "Owenweg 731", "zip": "4615 RB", "city": "Petten", "province": "Noord-Holland", "country": "NL", "contact": {"name": "Maud Adryaens", "phone": "+31836 752702", "email": "nickteunissen@example.com"}, "created_at": "2008-02-22 19:55:39", "updated_at": "2009-08-28 23:15:50"}]  
    _headers = {
        'API_KEY': 'a1b2c3d4e5', 
        'Content-Type': 'application/json'}   
    return _connection, _url, _testWarehouse, _headers

def test_post_warehouse(_DataPytestFixture):
    # Arrange
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    
    # Act
    # Add the two test warehouses to the database
    postStatuss = []
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST',_url, body=json_data, headers=_headers)
        response = _connection.getresponse()
        postStatuss.append(response.status)
        response.close()

    # Send a GET request to the GET_ALL endpoint
    _connection.request('GET', _url, headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouses = json.loads(data)
    # list comprehension to get a list of all warehouse id's
    warehousesIds = [w["id"] for w in warehouses]
    
    # Assert
    assert len(warehouses) >= 2
    for warehouse in _testWarehouses:
        assert warehouse["id"] in warehousesIds
    
    for status in postStatuss:
        assert status == 201

    # Clean up
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

def test_get_all(_DataPytestFixture):
    # Arrange
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    # Add the two test warehouses to the database
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST',_url, body=json_data, headers=_headers)
        _connection.getresponse().close()

    # Act
    # Send a GET request to the GET_ALL endpoint
    _connection.request('GET', _url, headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouses = json.loads(data)
    # list comprehension to get a list of all warehouse id's
    warehousesIds = [w["id"] for w in warehouses]
    
    # Clean up
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

    # Assert
    assert str(response.status) == '200'
    assert len(warehouses) >= 2
    for warehouse in _testWarehouses:
        assert warehouse["id"] in warehousesIds

        
def test_get_one(_DataPytestFixture):
    # Arrange
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    # Add the two test warehouses to the database
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST',_url, body=json_data, headers=_headers)
        _connection.getresponse().close()

    # Act   
    # Send a GET request to the GET_SPECIFIC_WAREHOUSE endpoint
    _connection.request('GET', f'{_url}/{_testWarehouses[0]["id"]}', headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouseAfter = json.loads(data)
    
    # Clean up
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

    # Assert
    assert str(response.status) == '200'
    assert type(warehouseAfter) == dict
    assert warehouseAfter["code"] == _testWarehouses[0]["code"]
    assert warehouseAfter["name"] == _testWarehouses[0]["name"]
    assert warehouseAfter["address"] == _testWarehouses[0]["address"]
    assert warehouseAfter["zip"] == _testWarehouses[0]["zip"]
    assert warehouseAfter["city"] == _testWarehouses[0]["city"]
    assert warehouseAfter["province"] == _testWarehouses[0]["province"]
    assert warehouseAfter["country"] == _testWarehouses[0]["country"]
    assert warehouseAfter["contact"] == _testWarehouses[0]["contact"]

def test_put_warehouse(_DataPytestFixture):
    # Arrange
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    extraWarehouse = {"id": 1000003, "code": "LIGMAL90", "name": "Petten shortterm hub", "address": "Owenweg 666", "zip": "6420 RB", "city": "Patten", "province": "Zuid-Holland", "country": "DE", "contact": {"name": "Maud Adryaens", "phone": "+31836 752702", "email": "nickteunissen@gmail.com"}, "created_at": "2021-02-22 19:55:39", "updated_at": "2009-08-28 23:15:50"}
    # Add the two test warehouses to the database
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST', f'{_url}', body=json_data, headers=_headers)
        _connection.getresponse().close()

    # Act
    # Update the first test warehouse with the extraWarehouse
    json_data = json.dumps(extraWarehouse).encode('utf-8')
    _connection.request('PUT',f'{_url}/{_testWarehouses[0]["id"]}', body=json_data, headers=_headers)
    response = _connection.getresponse()
    putStatusCode = response.status
    response.close()

    # Send a GET request to the GET_SPECIFIC_WAREHOUSE endpoint
    _connection.request('GET', f'{_url}/{extraWarehouse["id"]}', headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouseAfter = json.loads(data)
    
    # Clean up
    _connection.request('DELETE',f'{_url}/{extraWarehouse["id"]}', headers=_headers)
    _connection.getresponse().close()
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

    # Assert
    assert putStatusCode == 200
    assert warehouseAfter["code"] == extraWarehouse["code"]
    assert warehouseAfter["name"] == extraWarehouse["name"]
    assert warehouseAfter["address"] == extraWarehouse["address"]
    assert warehouseAfter["zip"] == extraWarehouse["zip"]
    assert warehouseAfter["city"] == extraWarehouse["city"]
    assert warehouseAfter["province"] == extraWarehouse["province"]
    assert warehouseAfter["country"] == extraWarehouse["country"]
    assert warehouseAfter["contact"] == extraWarehouse["contact"]

def test_delete_warehouse(_DataPytestFixture):
    # Arrange  
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    # Add the two test warehouses to the database
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST', f'{_url}', body=json_data, headers=_headers)
        _connection.getresponse().close()

    # Act  
    # Delete the first warehouse
    _connection.request('DELETE',f'{_url}/{_testWarehouses[0]["id"]}', headers=_headers)
    response = _connection.getresponse()
    deleteStatusCode = response.status
    response.close()

    # Send a GET request to the GET_ALL endpoint
    _connection.request('GET', f'{_url}', headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehousesDict = json.loads(data)

    # Try getting the deleted warehouse
    _connection.request('GET', f'{_url}/{_testWarehouses[0]["id"]}', headers=_headers)
    response = _connection.getresponse()
    warehouseAfter = response.read()
    
    # Clean up
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

    # Assert
    # "b'null'" Means that no warehouse was found
    assert str(warehouseAfter) == "b'null'"
    assert deleteStatusCode == 200
    
def test_add_warehouse_wrong_format(_DataPytestFixture):
    # Arrange  
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    
    warehouseWrongFormat = {"id": 1000003, "Wcode": "LIGMAL90", "Sname": "Petten shortterm hub", "adress": "Owenweg 666", "sip": "6420 RB", "citty": "Patten", "brovince": "Zuid-Holland", "countri": "DE", "cuntact": {"Sname": "Maud Adryaens", "Bhone": "+31836 752702", "Bmail": "nickteunissen@gmail.com"}, "created_at": "2021-02-22 19:55:39", "updated_at": "2009-08-28 23:15:50"}    
    
    # Act
    # Try adding the warehouse with the wrong format
    json_data = json.dumps(warehouseWrongFormat).encode('utf-8')
    _connection.request('POST',_url, body=json_data, headers=_headers)
    response = _connection.getresponse()
    postStatus = response.status
    response.close()

    # Send a GET request to the GET_ALL endpoint
    _connection.request('GET', _url, headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouses = json.loads(data)
    # list comprehension to get a list of all warehouse id's
    warehousesIds = [w["id"] for w in warehouses]

    # Clean up
    _connection.request('DELETE',f'{_url}/{warehouseWrongFormat["id"]}', headers=_headers)
    _connection.getresponse().close()

    # Assert
    assert postStatus == 400
    assert warehouseWrongFormat["id"] not in warehousesIds

def test_put_warehouse_wrong_format(_DataPytestFixture):
    # Arrange
    _connection, _url, _testWarehouses, _headers = _DataPytestFixture
    extraWrongWarehouse = {"id": 1000003, "Wcode": "LIGMAL90", "Sname": "Petten shortterm hub", "adress": "Owenweg 666", "sip": "6420 RB", "citty": "Patten", "brovince": "Zuid-Holland", "countri": "DE", "cuntact": {"Sname": "Maud Adryaens", "Bhone": "+31836 752702", "Bmail": "nickteunissen@gmail.com"}, "created_at": "2021-02-22 19:55:39", "updated_at": "2009-08-28 23:15:50"}
    # Add the two test warehouses to the database
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('POST', f'{_url}', body=json_data, headers=_headers)
        _connection.getresponse().close()

    # Act
    # Update the first test warehouse with the extraWarehouse
    json_data = json.dumps(extraWrongWarehouse).encode('utf-8')
    _connection.request('PUT',f'{_url}/{_testWarehouses[0]["id"]}', body=json_data, headers=_headers)
    response = _connection.getresponse()
    putStatusCode = response.status
    response.close()

    # Send a GET request to the GET_ALL endpoint
    _connection.request('GET', _url, headers=_headers)
    response = _connection.getresponse()
    data = response.read()
    warehouses = json.loads(data)
    # list comprehension to get a list of all warehouse id's
    warehousesIds = [w["id"] for w in warehouses]
    
    # Clean up
    _connection.request('DELETE',f'{_url}/{extraWrongWarehouse["id"]}', headers=_headers)
    _connection.getresponse().close()
    for warehouse in _testWarehouses:
        json_data = json.dumps(warehouse).encode('utf-8')
        _connection.request('DELETE',f'{_url}/{warehouse["id"]}', headers=_headers)
        _connection.getresponse().close()

    # Assert
    assert putStatusCode == 400
    assert extraWrongWarehouse["id"] not in warehousesIds

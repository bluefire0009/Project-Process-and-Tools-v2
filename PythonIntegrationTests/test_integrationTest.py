import pytest
import http.client
from GlobalVariables import BASE_URL, PORT, VERSION

# Define the base URL and the server (you can change it to your actual server address)


@pytest.fixture
def connection():
    # Create a connection to the server
    conn = http.client.HTTPConnection(BASE_URL, PORT)
    yield conn
    conn.close()


def test_simple_endpoint(connection: http.client.HTTPConnection):
    # Arrange
    link = VERSION
    location = {
        "name": "string",
        "address": "string",
        "city": "string",
        "zipCode": "string",
        "province": "string",
        "country": "string",
        "contactName": "string",
        "contactPhone": "string",
        "contactEmail": "user@example.com",
    }

    # Act
    connection.request("POST", f'{link}/clients/')
    response = connection.getresponse()

    # Assert
    assert response.status == 200

import pytest
import http.client

# Define the base URL and the server (you can change it to your actual server address)
BASE_URL = "localhost"
PORT = 3000


@pytest.fixture
def connection():
    # Create a connection to the server
    conn = http.client.HTTPConnection(BASE_URL, PORT)
    yield conn
    conn.close()


def test_simple_endpoint(connection: http.client.HTTPConnection):
    # Arrange
    url = "/api/v2/Test"

    # Act
    connection.request("GET", url)
    response = connection.getresponse()

    # Assert
    assert response.status == 500
    response_data = response.read().decode()
    assert response_data == "1"

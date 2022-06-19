import json
import pytest
import sorting_service


@pytest.fixture
def client(request):
    sorting_service.app.config['TESTING'] = True
    client = sorting_service.app.test_client()

    return client


def test_sort_req(client):
    """tests sorting request"""
    post_data = {
        "sortKeys": ["fruits", "numbers"],
        "payload": {
            "fruits": ["watermelon", "apple", "pineapple"],
            "numbers": [1333, 4, 2431, 7],
            "colors": ["green", "blue", "yellow"]
        }
    }
    expected_data = {
        "fruits": ["apple", "pineapple", "watermelon"],
        "numbers": [4, 7, 1333, 2431],
        "colors": ["green", "blue", "yellow"]
    }

    rv = client.post('/sort',
                     data=json.dumps(post_data),
                     headers={'content-type': 'application/json'})
    response_data = json.loads(rv.data.decode('utf-8'))

    assert response_data == expected_data


def test_sort_missing_key_sorts_all(client):
    """tests if request sorts all keys if sortKeys are missing"""
    post_data = {
        "payload": {
            "fruits": ["watermelon", "apple", "pineapple"],
            "numbers": [1333, 4, 2431, 7],
            "colors": ["green", "blue", "yellow"]
        }
    }
    expected_data = {
        "fruits": ["apple", "pineapple", "watermelon"],
        "numbers": [4, 7, 1333, 2431],
        "colors": ["blue", "green", "yellow"]
    }

    rv = client.post('/sort',
                     data=json.dumps(post_data),
                     headers={'content-type': 'application/json'})
    response_data = json.loads(rv.data.decode('utf-8'))

    assert response_data == expected_data


def test_sort_missing_payload_fail(client):
    """tests if request fails for arrays not provided for sorting"""
    post_data = {
        "sortKeys": ["fruits", "numbers"]
    }
    expected_data = {
        "err": "missing_payload"
    }

    rv = client.post('/sort',
                     data=json.dumps(post_data),
                     headers={'content-type': 'application/json'})
    response_data = json.loads(rv.data.decode('utf-8'))

    assert rv.status_code == 400
    assert response_data == expected_data


def test_sort_missing_keys_fail(client):
    """tests if request fails for keys not provided for sorting"""
    post_data = {
        "sortKeys": ["fruits", "trees"],
        "payload": {
            "fruits": ["watermelon", "apple", "pineapple"]
        }
    }
    expected_data = {
        "err": "invalid_params"
    }

    rv = client.post('/sort',
                     data=json.dumps(post_data),
                     headers={'content-type': 'application/json'})
    response_data = json.loads(rv.data.decode('utf-8'))

    assert rv.status_code == 400
    assert response_data == expected_data

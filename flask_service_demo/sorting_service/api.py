"""
Exposes a simple HTTP API to provide sorting services
"""
from flask import Flask, jsonify, request

app = Flask(__name__)


@app.route("/sort", methods=['POST'])
def sort():
    data = request.get_json()

    if 'payload' not in data:
        return jsonify({"err": "missing_payload"}), 400

    payload = data['payload']
    sort_keys = set(data.get('sortKeys')) if 'sortKeys' in data else set(payload.keys())
    response = {}

    for key, array in payload.items():
        response[key] = array

        # missing sort_keys assumes that all arrays are to be sorted
        if key in sort_keys:
            response[key].sort()
            sort_keys.remove(key)

    if sort_keys:
        # unhandled sort_keys means client provided incorrect array ref
        return jsonify({"err": "invalid_params"}), 400

    return jsonify(response)


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=9876)

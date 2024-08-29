from flask import Flask, send_from_directory, abort
import os

app = Flask(__name__)
BASE_DIR = input("Enter your data download directory name: ")

@app.route('/<path:filename>', methods=['GET'])
def download_file(filename):
    safe_path = os.path.join(BASE_DIR, filename)
    if not os.path.commonpath([BASE_DIR, safe_path]) == BASE_DIR:
        abort(404) 

    if os.path.isfile(safe_path):
        return send_from_directory(BASE_DIR, filename)
    else:
        abort(404)

if __name__ == '__main__':
    app.run(debug=True)

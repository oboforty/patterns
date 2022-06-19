adduser doors
sudo usermod -aG sudo doors

sudo apt update
sudo apt install -y nginx

echo 'from flask import Flask' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo '' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo 'app = Flask(__name__)' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo '' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo '@app.route("/")' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo 'def hello_world():' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo '    return "<p>Ciao, Bella!</p>"' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo 'if __name__ == "__main__":' | sudo tee -a /home/doors/wsgi.py > /dev/null
echo '    app.run()' | sudo tee -a /home/doors/wsgi.py > /dev/null


echo 'server {' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    server_name mywebsite.com;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    location ~*  \.(jpg|jpeg|png|gif|ico|css|js|pdf|woff|woff2|ttf)$ {' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '       # include  /etc/nginx/mime.types;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '       expires 30d;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '       root /home/doors/webapp/public/;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    }' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    location / {' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '        include proxy_params;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '        proxy_pass http://unix:/home/doors/doors.sock;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    }' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '    listen 80;' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null
echo '}' | sudo tee -a /etc/nginx/sites-available/doors.conf  > /dev/null

echo '[Unit]' | sudo tee -a /etc/systemd/system/doors.service > /dev/null 
echo 'Description=doors auth server' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'After=network.target' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo '' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo '[Service]' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'User=doors' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'Group=sudo' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'WorkingDirectory=/home/doors' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
#echo 'Environment="PATH=/home/doors/doorsenv/bin"' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'ExecStart=/usr/local/bin/gunicorn --workers 3 --bind unix:doors.sock -m 007 wsgi:app' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo '' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo '[Install]' | sudo tee -a /etc/systemd/system/doors.service > /dev/null
echo 'WantedBy=multi-user.target' | sudo tee -a /etc/systemd/system/doors.service > /dev/null


sudo ln -s /etc/nginx/sites-available/doors.conf /etc/nginx/sites-enabled
sudo rm /etc/nginx/sites-enabled/default
sudo nginx -t

# ======================================

sudo apt install -y python3-pip
sudo pip3 install eme
sudo apt install -y gunicorn

# ======================================

sudo systemctl daemon-reload
sudo systemctl enable doors
sudo systemctl stop nginx
sudo systemctl start doors
sudo systemctl stop doors


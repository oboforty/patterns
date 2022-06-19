# !/usr/bin/env python
import os
import zipfile


def zipdir(ziph, path, loc=None, exclude=None):
    nlist = ziph.namelist()

    # ziph is zipfile handle
    for root, dirs, files in os.walk(path):
        for file in files:
            name = root.replace('\\', '/') + '/' + file
            dirn = os.path.dirname(name)

            if '__pycache__' in name or '.log' in name or (exclude and dirn in exclude):
                continue

            if loc:
                lname = loc + '/' + file
                if lname not in nlist:
                    ziph.write(name, lname)
            else:
                if name not in nlist:
                    ziph.write(name)


class DeployCommand():
    def __init__(self, server):
        self.server = server
        try:
            self.conf = loadConfig('cliapp/deploy/ssh.ini')['ssh']
        except:
            pass
        self.commands = {
            'deploy': {
                'help': 'Uploads files',
                'short': {},
                'long': []
            },
        }

    def _upload(self):
        import paramiko

        print('Connecting to server...')
        ssh = paramiko.SSHClient()
        ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        # ssh.load_host_keys(os.path.expanduser(os.path.join("~", ".ssh", "known_hosts")))
        ssh.connect(self.conf['host'], username=self.conf['username'], password=self.conf['password'])
        sftp = ssh.open_sftp()

        print('Uploading file...')
        sftp.put('deployment.zip', self.conf['remotepath'] + '/deployment.zip')
        sftp.close()

        print('Installing...')
        stdin, stdout, stderr = ssh.exec_command("unzip -o {0}/deployment.zip -d {0}".format(self.conf['remotepath']))
        err = stderr.readlines()
        if err:
            print(err)
            return ssh.close()

        stdin, stdout, stderr = ssh.exec_command("rm -rf {0}/deployment.zip".format(self.conf['remotepath']))
        err = stderr.readlines()
        if err:
            print(err)
            return ssh.close()

        # print('Restarting...')
        # stdin, stdout, stderr = ssh.exec_command("systemctl restart geopoly")
        # err = stderr.readlines()
        # if err:
        #     print(err)
        #     return ssh.close()
        #
        # stdin, stdout, stderr = ssh.exec_command("systemctl status geopoly")
        # err = stderr.readlines()
        # if err:
        #     print(err)
        #     return ssh.close()
        # print(stdout.readlines())

        ssh.close()

    def run(self):
        # auto increment version
        with open('game/content/version.txt') as fh:
            version = fh.readline()
        with open('game/content/version.txt', 'w') as fh:
            fh.write(str(int(version)+1))


        print('Packing...')
        zipf = zipfile.ZipFile('deployment.zip', 'w', zipfile.ZIP_DEFLATED)

        # Overwrite config files
        B = loadConfig('cliapp/config.ini')['deploy']['content_source']
        zipf.write(B+'ctx.ini', 'game/content/ctx.ini')
        zipf.write(B+'game.ini', 'game/content/config.ini')
        zipf.write(B+'website.ini', 'webapp/config.ini')
        zipf.write(B+'cli.ini', 'cliapp/config.ini')

        zipdir(zipf, 'game')
        zipdir(zipf, 'engine')
        zipdir(zipf, 'cliapp')
        zipdir(zipf, 'webapp', exclude=[
            'webapp/public/models/buildings',
            'webapp/public/models/test',
            'webapp/public/img/textures/skies',
            'webapp/public/img/textures/terrain',
            'webapp/public/img/textures/normals',
            'webapp/public/legal',
            'webapp/public/sounds',
            'webapp/public/client/gfx',
            'webapp/public/client/gfx2d',
        ])

        zipf.write('wsgi.py', 'wsgi.py')
        zipf.write('cli.py', 'cli.py')

        zipf.close()

        self._upload()

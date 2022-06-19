
import json
import os
import sys
from ftplib import FTP

with open('nodes.json') as nodejson:
	nodes = json.load(nodejson)

	for node in nodes:
		try:
			ftp = FTP('ftp.atw.hu')
			ftp.login(user=node['name'], passwd=node['password'])
			oldfiles = ftp.nlst()
			print('Deleting files')

			if 'lnode.instance' not in oldfiles:
				for file in oldfiles:
					try:
						ftp.delete(file)
					except:
						continue

			# Setup database config!

			print('Parsing src')
			for dirpath, dirnames, filenames in os.walk('src'):
				rdirpath = dirpath.replace('src', '').replace('\\', '/')
				print('* Copying', rdirpath)

				if not rdirpath == '':
					try:
						ftp.mkd(rdirpath)
					except:
						pass

				for filename in filenames:
					filepath = os.path.join(dirpath, filename)
					rfilepath = os.path.join(rdirpath, filename).replace('\\', '/')

					print(' -- ', filepath, rfilepath)
					
					with open(filepath, 'rb') as myfile:
						try:
							ftp.storlines('STOR ' + rfilepath, myfile)
						except:	
							pass
			ftp.quit()
		except Exception as e:
			exc_type, exc_obj, exc_tb = sys.exc_info()
			print(exc_type, exc_obj, exc_tb.tb_lineno)
			pass
import os

from flask import render_template, request, Response, url_for

from core.celery import celery_service
from webapi.entities import ApiResponse



class HomeController():
    def __init__(self, server):
        self.server = server
        self.group = "Home"

        self.server.addUrlRule({
            'GET /add/<int:a>+<int:b>': 'home/add_numbers',
            'GET /pronouns/<int:nfiles>': 'home/count_pronouns',
            'GET /pronouns/track/<task_id>': 'home/task_status',
        })

    def index(self):

        return render_template('/home/index.html')


    def add_numbers(self,a,b):
        n = celery_service.add_two_numbers_sync(a, b)

        return ApiResponse({
            "result": "OK",
            "numbers": [a, b],
            "add": n
        })

    def get_count_pronouns(self, nfiles=1):
        bdir = "/home/ubuntu/data/"
        files = []
        is_async = request.args.get('async', 'false').lower() == 'true'

        for i,f in enumerate(os.listdir(bdir)):
            if i+1 > nfiles:
                break

            files.append(os.path.join(bdir, f))

        # return ApiResponse({
        #     "result": "progress",
        #     "files": files,
        # })

        if is_async:
            task_id = celery_service.pronouns_start(files)

            return ApiResponse({
                "result": "progress",
                "task": task_id,
                "url": url_for("get__home/task_status", task_id=task_id)
            })

        else:
            pronouns = celery_service.pronouns_sync(files)

            return ApiResponse({
                "result": "OK",
                "counts": pronouns
            })

    def get_task_status(self, task_id=None):
        if task_id is None:
            return ApiResponse({
                "task": task_id,
                "error": "task_not_found",
            })

        finished, resp = celery_service.check_task(task_id)

        if finished is None:
            return ApiResponse({
                "task": task_id,
                "error": "task_not_found",
            })

        if finished:
            return ApiResponse({
                "task": task_id,
                "status": "SUCCESS",
                "result": resp,
            })

        else:
            return ApiResponse({
                "task": task_id,
                "status": resp,
            })

from .tasks import add, count_pronouns


def add_two_numbers_sync(a, b):
    # call celery to test

    result = add.delay(a, b)
    result.wait()

    return result.result


def pronouns_sync(files):
    # call celery to count pronouns

    result = count_pronouns.delay(files)
    result.wait()

    return result.result


def pronouns_start(files):
    # starts an async task
    task = count_pronouns.apply_async((files,), retry=False)

    return task.id


def check_task(task_id):
    # Checks status if incomplete, or retrieves the result
    task = count_pronouns.AsyncResult(task_id)

    if task is None:
        return None, None

    if task.state == 'SUCCESS':
        return True, task.get()

    return False, task.state

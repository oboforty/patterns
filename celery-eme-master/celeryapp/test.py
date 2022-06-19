from core.celery.tasks import add

result = add.delay(10, 12)
print(result.result)


def on_raw_message(body):
    print(body['result'])

r = add.apply_async((4,4), retry=False)
r.get(on_message=on_raw_message, propagate=False)

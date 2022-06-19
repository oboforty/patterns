from collections import defaultdict

from celery import Celery
from core.celery import celeryconfig

from core.tweeter import count_tweets

app = Celery('tasks')
app.config_from_object(celeryconfig)


@app.task
def add(x, y):
    return x + y


@app.task
def count_pronouns(files):
    pronoun_counts = defaultdict(int)

    # summarize for all files:
    for file in files:
        result = count_tweets(file)
        for k,v in result.items():
            pronoun_counts[k] += v

    return dict(pronoun_counts)

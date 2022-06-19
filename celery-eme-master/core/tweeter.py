import json
from collections import Counter, defaultdict

pronouns = ["han", "hon", "den", "det", "denna", "denne", "hen"]

def count_tweets(file):
    pronoun_counts = defaultdict(int)

    with open(file) as fh:
        for line in fh:
            line = line.strip()

            if not line:
                continue

            try:
                tweet = json.loads(line)

                if tweet['retweeted']:
                    # disregard retweets
                    continue

                # split tweet content into words & calculate word frequencies
                words = tweet["text"].lower().split()
                word_freq = Counter(words)

                for pronoun in pronouns:
                    pronoun_counts[pronoun] += word_freq.get(pronoun, 0)

                # uncomment for testing
                # break

            except Exception as e:
                # just in any case (EAFP):
                print('%s\t%s' % (str(e), 1))

                continue

    return dict(pronoun_counts)


if __name__ == "__main__":

    r = count_tweets('raw/data/0c7526e6-ce8c-4e59-884c-5a15bbca5eb3')

    print(r)

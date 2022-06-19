import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

"""
Control histogram to check against
"""
#df = pd.DataFrame( np.random.randint(1, 7, (20000,4)), columns = ['dice11', 'dice12', 'dice21', 'dice22'])

"""
CSV (date|stake|user|outcome|dice11|dice12|dice21|dice22)
  filter out double throws
"""
df = pd.read_csv('data.csv', sep='|')
df = df[(df['dice21'] != 0) & (df['dice22'] != 0)]
del df['stake']

kw = {
    'method': 'spearman'
}

print(df.corr(**kw))

plt.matshow(df.corr(**kw))
plt.show()


print("--------")
print((df['dice11'] + df['dice12']).corr((df['dice21'] + df['dice22'])))
print((df['dice11'] + df['dice21']).corr((df['dice12'] + df['dice22'])))
print((df['dice11'] + df['dice22']).corr((df['dice12'] + df['dice21'])))

import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

"""
Control histogram to check against
"""
#df = pd.DataFrame( np.random.randint(1, 7, (6000,4)), columns = ['dice11', 'dice12', 'dice21', 'dice22'])

"""
CSV (date|stake|user|outcome|dice11|dice12|dice21|dice22)
  filter out double throws
"""
df = pd.read_csv('data.csv', sep='|')
df = df[(df['dice21'] != 0) & (df['dice22'] != 0)]


df['sum1'] = df['dice11'] + df['dice12']
df['sum2'] = df['dice21'] + df['dice22']

ax = df['sum1'].plot.hist(bins=10)
plt.show()

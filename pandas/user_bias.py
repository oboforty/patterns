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
del df['stake']


gg = df[df['user'] == 'WorldTeller']

print(gg.corr())

print(gg['outcome'].value_counts())

# plt.matshow(gg.corr(other=gg['outcome']))
# plt.show()

import pandas as pd
import matplotlib.pyplot as plt


outcomes = df['outcome'].value_counts()

df['sum1'] = df['dice11'] + df['dice12']
df['sum2'] = df['dice21'] + df['dice22']

ax = df['sum2'].plot.hist(bins=10)
plt.show()

# pd.plotting.scatter_matrix(df, alpha=0.2)
# plt.show()
#
# ax = df.plot(kind='scatter', x='outcome', y='dice11', color='r')
# ax4 = df.plot(kind='scatter', x='outcome', y='dice12', color='g', ax=ax)
# ax2 = df.plot(kind='scatter', x='outcome', y='dice21', color='b', ax=ax)
# ax3 = df.plot(kind='scatter', x='outcome', y='dice22', color='m', ax=ax)
#
# ax.set_xlabel("First dice throw")
# ax.set_ylabel("Other 3 dice throws")
# plt.show()

# d1_results = df['dice11'].value_counts()
# d2_results = df['dice12'].value_counts()
# d3_results = df['dice21'].value_counts()
# d4_results = df['dice22'].value_counts()

print(1)
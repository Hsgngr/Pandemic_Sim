# -*- coding: utf-8 -*-
"""
Created on Tue Jul 28 17:48:26 2020

@author: Ege
"""
# Importing the libraries
import seaborn as sns
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.cluster import KMeans

# Styling
sns.set()
blue = sns.color_palette("muted", 1)


# Importing the dataset
dataset=pd.read_csv('export.csv', sep=';')
dataset['Time'] = dataset['Time'].str.replace(',','.')

def exportAnimation(data,number_of_frame,cluster_numbers):
    for i in range(0,5212,100):      
        dataset=data.iloc[:i+100,:] 
        x=data['HealthyCount']
        df = pd.DataFrame(data = dataset['Time'])
        df.insert(1,'HealthyCount',x)
        
        cluster_number = 10
        kmeans = KMeans(n_clusters=cluster_number)
        kmeans.fit(df)
        y_kmeans = kmeans.predict(df)
        centers = kmeans.cluster_centers_
        centers=np.around(centers,decimals=1)
        
        centers = pd.DataFrame(data= centers)
        centers.rename(columns={0:'time',1:'HealthyCount'}, inplace=True)
        centers.sort_values(by=['time'], inplace=True)
        centers= centers.to_numpy()    
        
        x=centers[:,0]
        y=centers[:,1]
        
        plt.plot(x,y)
        plt.savefig("Graph" + str(i) +".png", format="PNG")

exportAnimation(dataset,500,100)

for i in range(0,5212,100):
    print(i)
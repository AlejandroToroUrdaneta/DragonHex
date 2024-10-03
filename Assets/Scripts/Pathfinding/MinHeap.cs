using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IHeapItem<T> {
    T[] items;
    int currentItemCount;

    public MinHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];

    }

    public void CreateMinHeap(T[] v)
    {
        items = new T[v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            items[i + 1] = v[i];
        }
        currentItemCount = v.Length;
        FixMinHeap();
    }

    private void FixMinHeap()
    {
        for (int i = currentItemCount / 2; i > 0; i--)
        {
            SortDown(items[i]);
        }
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }
    
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get{
            return currentItemCount;
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if(childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if(childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareByAttributeIndex(items[childIndexRight]) < 0) {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareByAttributeIndex(items[swapIndex]) < 0)
                {
                    swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if(item.CompareByAttributeIndex(parentItem) > 0)
            {
                swap(item,parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }


    }

    void swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T>: ICustomComparable<T>{
    int HeapIndex{
        get;
        set;
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeapOfInfluence<T> where T : IHeapItemInfluence<T>
{
    T[] items;
    int currentItemCount;

    public MinHeapOfInfluence(int maxHeapSize)
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
        item.HeapIndexInfluence = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndexInfluence = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public T CheckFirst()
    {
        T firstItem = items[0];
        return firstItem;
    }

    public void Remove(int index)
    {
        currentItemCount--;
        T item = items[currentItemCount];
        items[index] = item;
        items[index].HeapIndexInfluence = index;
        int parentIndex = (items[index].HeapIndexInfluence - 1) / 2;
        T parentItem = items[parentIndex];
        if (items[index].CompareByAttributeInfluence(parentItem) > 0)
        {
            SortUp(items[index]);
        }
        else
        {
            SortDown(items[index]);
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndexInfluence], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndexInfluence * 2 + 1;
            int childIndexRight = item.HeapIndexInfluence * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareByAttributeInfluence(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareByAttributeInfluence(items[swapIndex]) < 0)
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
        int parentIndex = (item.HeapIndexInfluence - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareByAttributeInfluence(parentItem) > 0)
            {
                swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndexInfluence - 1) / 2;
        }


    }

    void swap(T itemA, T itemB)
    {
        items[itemA.HeapIndexInfluence] = itemB;
        items[itemB.HeapIndexInfluence] = itemA;
        int itemAIndex = itemA.HeapIndexInfluence;
        itemA.HeapIndexInfluence = itemB.HeapIndexInfluence;
        itemB.HeapIndexInfluence = itemAIndex;
    }
}

public interface IHeapItemInfluence<T> : ICustomComparable<T> 
{
    int HeapIndexInfluence
    {
        get;
        set;
    }

}
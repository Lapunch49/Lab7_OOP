using System;
using System.Collections.Generic;
using System.Text;

namespace Lab7_OOP
{
    public class Storage
    {
        private int n, k; // размер и кол-во эл-в
        public CObject[] st;
        public Storage()
        {
            n = 1;
            st = new CObject[n];
            st[0] = null; // или default
            k = 0;
        }
        public Storage(int size)
        {
            n = size;
            st = new CObject[n];
            k = 0;
            for (int i = 0; i < n; ++i)
                st[i] = null;
        }
        public void add(CObject new_el)
        {
            if (k < n)
            {
                st[k] = new_el;
                k = k + 1;
            }
            else
            {
                n = n * 2;
                CObject[] st_ = new CObject[n];
                for (int i = 0; i < k; ++i)
                    st_[i] = st[i];
                st_[k] = new_el;
                k = k + 1;
                for (int i = k; i < n; ++i)
                    st_[i] = null;
                st = st_;
            }
        }
        public void del(int ind)
        {
            for (int i = ind; i < k - 1; ++i)
                st[i] = st[i + 1];
            k = k - 1;
            st[k] = null;
        }
        public CObject get_el(int ind)
        {
            return st[ind];
        }
        public int get_count()
        {
            return k;
        }
    };
}

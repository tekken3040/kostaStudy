package com.kosta.lec07viewacivitytotal;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentStatePagerAdapter;
import androidx.viewpager.widget.ViewPager;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import java.util.ArrayList;


public class Fragment1 extends Fragment {

    private static final String Tag = "Fragment1";



    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        ViewGroup viewGroup = (ViewGroup)inflater.inflate(R.layout.fragment1, container, false);

        Button btnNext = (Button)viewGroup.findViewById(R.id.btnNext);
        btnNext.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //
            }
        });

        ViewPager pager = viewGroup.findViewById(R.id.pager);
        pager.setOffscreenPageLimit(3);

        CustomerPagerAdapter adapter =
                new CustomerPagerAdapter(getFragmentManager());
        for(int i = 0; i < 3; i++)
        {
            PageFragment pageFragment = createPage(i);
            adapter.addItem(pageFragment);
        }
        pager.setAdapter(adapter);

        return  viewGroup;

    }
    public PageFragment createPage(int index){
        String name ="화면 "+ index;
        PageFragment fragment = PageFragment.newInstance(name);
        return fragment;
    }
    class  CustomerPagerAdapter extends FragmentStatePagerAdapter{
        ArrayList<Fragment> items = new ArrayList<Fragment>();

        public CustomerPagerAdapter(FragmentManager fm) {
            super(fm);
        }

        public void addItem(Fragment item){
            items.add(item);
        }

        @Override
        public Fragment getItem(int position) {
            return items.get(position);
        }

        @Override
        public int getCount() {
            return items.size();
        }
    }

}

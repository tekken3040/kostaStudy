package com.example.kosta30viewactivitytotal;


import android.content.Context;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentStatePagerAdapter;
import androidx.viewpager.widget.ViewPager;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import java.util.ArrayList;


/**
 * A simple {@link Fragment} subclass.
 */
public class Fragment1 extends Fragment
{
    private static final String tag = "Fragment1";
    FragmentCallback fragmentCallback;

    @Override
    public void onAttach(@NonNull Context context)
    {
        super.onAttach(context);
        if(context instanceof FragmentCallback)
        {
            fragmentCallback = (FragmentCallback)context;
        }
        else
        {
            Log.d("TAG", "액티비티가 프래그먼트 콜백을 가지고있지 않다.");
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater,ViewGroup container,Bundle savedInstanceState)
    {
        ViewGroup viewGroup = (ViewGroup)inflater.inflate(R.layout.fragment1, container, false);
        Button btnNext = (Button)viewGroup.findViewById(R.id.btnNext);
        btnNext.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                fragmentCallback.onFragmentSelected(1, null);
            }
        });

        ViewPager pager = viewGroup.findViewById(R.id.pager);
        pager.setOffscreenPageLimit(3);

        CustomPagerAdapter adapter = new CustomPagerAdapter(getFragmentManager());

        for(int i=0; i<3; i++)
        {
            Fragment4 pageFragment = createPage(i);
            adapter.addItem(pageFragment);
        }
        pager.setAdapter(adapter);
        return viewGroup;
    }

    public Fragment4 createPage(int index)
    {
        String name = "화면"+index;
        Fragment4 fragment = Fragment4.newInstance(name);
        return fragment;
    }

    class CustomPagerAdapter extends FragmentStatePagerAdapter
    {
        ArrayList<Fragment> items = new ArrayList<Fragment>();

        public CustomPagerAdapter(FragmentManager fm)
        {
            super(fm);
        }

        public void addItem(Fragment item)
        {
            items.add(item);
        }

        @Override
        public Fragment getItem(int position)
        {
            return items.get(position);
        }

        @Override
        public int getCount()
        {
            return items.size();
        }
    }
}

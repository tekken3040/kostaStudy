package com.kosta.lec07viewacivitytotal;


import android.os.Bundle;

import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.viewpager.widget.ViewPager;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;


public class PageFragment extends Fragment {

    TextView textView;
    String name;

    public static  PageFragment newInstance(String name){
        PageFragment fragment = new PageFragment();
        Bundle bundle = new Bundle();
        bundle.putString("name",name);
        fragment.setArguments(bundle);
        return fragment;
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Bundle bundle = getArguments();
        name = bundle.getString("name");
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
       ViewGroup viewGroup = (ViewGroup) inflater.inflate(R.layout.page, container, false);
       textView = viewGroup.findViewById(R.id.txtPage);
       textView.setText(name);
       return  viewGroup;



    }

}

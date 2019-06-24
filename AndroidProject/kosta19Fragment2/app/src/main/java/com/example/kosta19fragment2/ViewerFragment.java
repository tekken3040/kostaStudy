package com.example.kosta19fragment2;


import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;


/**
 * A simple {@link Fragment} subclass.
 */
public class ViewerFragment extends Fragment
{
    ImageView imageView;

    public ViewerFragment()
    {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater,ViewGroup container,Bundle savedInstanceState)
    {
        // Inflate the layout for this fragment
        ViewGroup viewGroup = (ViewGroup)inflater.inflate(R.layout.fragment_viewer,container,false);
        imageView = viewGroup.findViewById(R.id.imageView);

        return viewGroup;
    }

    public void setImageView(int resID)
    {
        imageView.setImageResource(resID);
    }
}

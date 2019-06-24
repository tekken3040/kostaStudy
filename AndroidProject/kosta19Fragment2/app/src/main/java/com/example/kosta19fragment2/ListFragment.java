package com.example.kosta19fragment2;


import android.content.Context;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;


/**
 * A simple {@link Fragment} subclass.
 */
public class ListFragment extends Fragment
{
    @Override
    public void onAttach(Context context)
    {
        super.onAttach(context);
        if(context instanceof ImageSelectionCallback)
        {
            callback = (ImageSelectionCallback)context;
        }
    }

    public static interface ImageSelectionCallback
    {
        public void onImageSelected(int position);
    }

    public ImageSelectionCallback callback;

    @Override
    public View onCreateView(LayoutInflater inflater,ViewGroup container,Bundle savedInstanceState)
    {
        // Inflate the layout for this fragment
        ViewGroup viewGroup = (ViewGroup)inflater.inflate(R.layout.fragment_list,container,false);
        Button btnOne = (Button)viewGroup.findViewById(R.id.btnOne);
        Button btnTwo = (Button)viewGroup.findViewById(R.id.btnTwo);
        Button btnThree = (Button)viewGroup.findViewById(R.id.btnThree);

        btnOne.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(callback != null)
                    callback.onImageSelected(0);
            }
        });

        btnTwo.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(callback != null)
                    callback.onImageSelected(1);
            }
        });

        btnThree.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(callback != null)
                    callback.onImageSelected(2);
            }
        });

        return viewGroup;
    }
}

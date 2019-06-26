package com.example.kosta30viewactivitytotal;


import android.content.Context;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;


/**
 * A simple {@link Fragment} subclass.
 */
public class Fragment3 extends Fragment
{
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
        // Inflate the layout for this fragment
        ViewGroup viewGroup = (ViewGroup)inflater.inflate(R.layout.fragment3,container,false);
        Button btnThird = (Button)viewGroup.findViewById(R.id.btnThird);
        btnThird.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                fragmentCallback.onFragmentSelected(0, null);
            }
        });

        return viewGroup;
    }
}

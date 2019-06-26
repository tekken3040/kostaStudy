package com.example.kosta30viewactivitytotal;


import android.os.Bundle;

import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;


/**
 * A simple {@link Fragment} subclass.
 */
public class Fragment4 extends Fragment
{
    TextView textView;
    String name;

    public static Fragment4 newInstance(String name)
    {
        Fragment4 fragment4 = new Fragment4();
        Bundle bundle = new Bundle();
        bundle.putString("name", name);
        fragment4.setArguments(bundle);

        return fragment4;
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        Bundle bundle = getArguments();
        name = bundle.getString("name");
    }

    @Override
    public View onCreateView(LayoutInflater inflater,ViewGroup container,Bundle savedInstanceState)
    {
        // Inflate the layout for this fragment
        ViewGroup viewGroup = (ViewGroup) inflater.inflate(R.layout.fragment4,container,false);
        textView = viewGroup.findViewById(R.id.txtPage);
        textView.setText(name);

        return viewGroup;
    }
}

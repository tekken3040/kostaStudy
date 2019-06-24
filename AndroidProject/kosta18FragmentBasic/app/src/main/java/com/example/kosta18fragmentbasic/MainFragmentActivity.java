package com.example.kosta18fragmentbasic;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import com.example.kosta18fragmentbasic.ui.main.MainFragment;

public class MainFragmentActivity extends AppCompatActivity
{

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main_activity);
        if(savedInstanceState==null)
        {
            getSupportFragmentManager().beginTransaction().replace(R.id.container,MainFragment.newInstance()).commitNow();
        }
    }
}

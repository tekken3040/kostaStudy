package com.example.kosta18fragmentbasic;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import com.example.kosta18fragmentbasic.ui.main.MainFragment;

public class MainActivity extends AppCompatActivity
{
    MainFragment mainFragment;
    MenuFragment menuFragment;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mainFragment = (MainFragment)getSupportFragmentManager().findFragmentById(R.id);
    }
}

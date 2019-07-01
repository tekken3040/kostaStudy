package com.example.kosta40recycleview;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.annotation.SuppressLint;
import android.os.Bundle;

public class MainActivity extends AppCompatActivity
{

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        RecyclerView recyclerView = findViewById(R.id.recyclerView);
        @SuppressLint("WrongConstant") LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.VERTICAL, false);
        recyclerView.setLayoutManager(linearLayoutManager);

        PersonAdapter adapter = new PersonAdapter();
        adapter.addItem(new Person("박명수", "010-2265-7789"));
        adapter.addItem(new Person("김경호", "010-2356-8899"));
        adapter.addItem(new Person("박완규", "010-6500-7978"));
        adapter.addItem(new Person("임재범", "010-7304-0089"));

        recyclerView.setAdapter(adapter);
    }
}

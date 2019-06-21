package com.kosta.kosta12thread;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class MainActivity extends AppCompatActivity
{
    Button btn_BasicThread, btn_BasicThread2, btn_PostThread, btn_LooperTread, btn_ProgressThread, btn_ProgressPostThread, btn_TimeoutThread, btn_CountDownThread, btn_AsyncThread;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btn_BasicThread = (Button)findViewById(R.id.btn_BasicTread);
        btn_BasicThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, BasicThread1.class);
                startActivity(intent);
            }
        });
        btn_BasicThread2 = (Button)findViewById(R.id.btn_BasicThread2);
        btn_BasicThread2.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, BasicThread2Activity.class);
                startActivity(intent);
            }
        });
        btn_LooperTread = (Button)findViewById(R.id.btn_LooperThread);
        btn_LooperTread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, LooperActivity.class);
                startActivity(intent);
            }
        });
        btn_ProgressThread = (Button)findViewById(R.id.btn_ProgressThread);
        btn_ProgressThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, ProgressActivity.class);
                startActivity(intent);
            }
        });
        btn_ProgressPostThread = (Button)findViewById(R.id.btn_ProgressPostThread);
        btn_ProgressPostThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, ProgressPostActivity.class);
                startActivity(intent);
            }
        });
        btn_TimeoutThread = (Button)findViewById(R.id.btn_TimeoutThread);
        btn_TimeoutThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, TimeoutActivity.class);
                startActivity(intent);
            }
        });
        btn_CountDownThread = (Button)findViewById(R.id.btn_CountDownThread);
        btn_CountDownThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, CountdownActivity.class);
                startActivity(intent);
            }
        });
        btn_AsyncThread = (Button)findViewById(R.id.btn_AsyncTaskThread);
        btn_AsyncThread.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, AsynTaskActivity.class);
                startActivity(intent);
            }
        });
    }
}

package com.example.kosta37webclient;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.FormBody;
import okhttp3.OkHttpClient;
import okhttp3.Request;

public class MainActivity extends AppCompatActivity
{
    EditText editFirst, editSecond;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editFirst = (EditText)findViewById(R.id.editFirst);
        editSecond = (EditText)findViewById(R.id.editSecond);
        Button btnRequest = (Button)findViewById(R.id.btnRequest);
        btnRequest.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                RequestThread requestThread = new RequestThread();
                requestThread.start();
            }
        });
    }

    class RequestThread extends Thread
    {
        @Override
        public void run()
        {
            super.run();
            try
            {
                OkHttpClient client = new OkHttpClient();
                Request.Builder builder = new Request.Builder();
                builder.url("http://106.242.203.69:2740/WebAndroid/upload.jsp");
                String mobile_str1 = editFirst.getText().toString();
                String mobile_str2 = editSecond.getText().toString();

                FormBody.Builder builder1 = new FormBody.Builder();
                builder1.add("mobile_str1", mobile_str1);
                builder1.add("mobile_str2", mobile_str2);
                FormBody body = builder1.build();
                builder = builder.post(body);
                Request request = builder.build();
                Call call = client.newCall(request);
                call.execute();
            }
            catch(IOException e)
            {
                e.printStackTrace();
            }
        }
    }
}

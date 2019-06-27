package com.example.kosta33httpnetwork;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.telecom.Call;
import android.view.View;
import android.widget.TextView;
import android.telecom.Call;

import org.jetbrains.annotations.NotNull;

import java.io.IOException;

import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

public class MainActivity extends AppCompatActivity
{
    TextView textView;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView = (TextView)findViewById(R.id.textView);
    }

    public void ConnectServerBtn(View view)
    {
        NetworkThread thread = new NetworkThread();
        thread.start();
    }

    class NetworkThread extends Thread
    {
        @Override
        public void run()
        {
            super.run();
            OkHttpClient client = new OkHttpClient();
            Request.Builder builder = new Request.Builder();
            builder = builder.url("http://google.com");
            Request request = builder.build();
            okhttp3.Call call = client.newCall(request);
            NetworkCallback callback = new NetworkCallback();
            (call).enqueue(callback);
        }
    }

    class NetworkCallback implements Callback
    {
        @Override
        public void onFailure(@NotNull okhttp3.Call call,@NotNull IOException e)
        {

        }

        @Override
        public void onResponse(@NotNull okhttp3.Call call,@NotNull Response response) throws IOException
        {
            try
            {
                final String result = response.body().string();
                runOnUiThread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        textView.setText(result);
                    }
                });
            }
            catch(IOException e)
            {
                e.printStackTrace();
            }
        }
    }
}

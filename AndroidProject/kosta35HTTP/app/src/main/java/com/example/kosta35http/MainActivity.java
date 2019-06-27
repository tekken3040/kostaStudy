package com.example.kosta35http;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

public class MainActivity extends AppCompatActivity
{
    EditText editURL;
    TextView txtResult;
    Handler handler = new Handler();

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editURL = (EditText)findViewById(R.id.editURL);
        txtResult = (TextView)findViewById(R.id.txtResult);
        Button btnRequest = (Button)findViewById(R.id.btnRequest);
        btnRequest.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                final String urlStr = editURL.getText().toString();
                new Thread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        request(urlStr);
                    }
                }).start();
            }
        });
    }

    public void request(String urlStr)
    {
        StringBuilder output = new StringBuilder();
        try
        {
            URL url = new URL(urlStr);
            HttpURLConnection connection = (HttpURLConnection)url.openConnection();
            if(connection != null)
            {
                connection.setConnectTimeout(10000);
                connection.setRequestMethod("GET");
                connection.setDoInput(true);
                int resCode = connection.getResponseCode();
                BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                String line = null;
                while(true)
                {
                    line = reader.readLine();
                    if(line == null)
                        break;
                    output.append(line+"\n");
                }
                reader.close();
                connection.disconnect();
            }
        }
        catch(IOException e)
        {
            println("에외 발생 : " + e.getMessage());
            e.printStackTrace();
        }
        println("응답->" + output.toString());
    }

    public void println(final String data)
    {
        handler.post(new Runnable()
        {
            @Override
            public void run()
            {
                txtResult.append(data+"\n");
            }
        });
    }
}

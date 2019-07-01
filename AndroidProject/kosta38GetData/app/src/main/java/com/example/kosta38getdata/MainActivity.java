package com.example.kosta38getdata;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import org.jetbrains.annotations.NotNull;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.FormBody;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

public class MainActivity extends AppCompatActivity
{
    EditText editID;
    TextView txtResult;
    Button btnRequest;
    String mobile_id;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editID = (EditText)findViewById(R.id.editID);
        txtResult = (TextView)findViewById(R.id.txtResult);
        btnRequest = (Button)findViewById(R.id.btnRequest);
        btnRequest.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                mobile_id = editID.getText().toString();//Integer.parseInt(editID.getText().toString());
                GetDataThread getDataThread = new GetDataThread();
                getDataThread.start();
            }
        });
    }

    class GetDataThread extends Thread
    {
        @Override
        public void run()
        {
            super.run();
            OkHttpClient client = new OkHttpClient();
            Request.Builder builder = new Request.Builder();
            builder = builder.url("http://106.242.203.69:2740/WebAndroid/get_data.jsp");//?mobile_id=1234
            FormBody.Builder bodyBuilder = new FormBody.Builder();

            bodyBuilder.add("mobile_id", mobile_id+"");
            FormBody body = bodyBuilder.build();
            builder = builder.post(body);
            Request request = builder.build();

            CallbackData callbackData = new CallbackData();
            Call call = client.newCall(request);
            call.enqueue(callbackData);
            //bodyBuilder.add("mobile_id", mobile_id+"");
        }
    }

    class CallbackData implements Callback
    {
        @Override
        public void onFailure(@NotNull Call call,@NotNull IOException e)
        {

        }

        @Override
        public void onResponse(@NotNull Call call,@NotNull Response response) throws IOException
        {
            try
            {
                final String result = response.body().string();
                JSONObject object = new JSONObject(result);
                final String name = object.getString("name");
                final String password = object.getString("userPassword");
                runOnUiThread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        txtResult.setText("");
                        txtResult.append("이름 : " + name + "\n");
                        txtResult.append("비밀번호 : " + password + "\n");
                    }
                });
            }
            catch(JSONException e)
            {
                e.printStackTrace();
            }
        }
    }
}

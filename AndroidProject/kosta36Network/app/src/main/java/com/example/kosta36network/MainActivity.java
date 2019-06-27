package com.example.kosta36network;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import com.android.volley.AuthFailureError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.Response.ErrorListener;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import java.util.HashMap;
import java.util.Map;

public class MainActivity extends AppCompatActivity
{
    EditText editURL;
    TextView txtResult;
    static RequestQueue requestQueue;
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
                makeRequest();
            }
        });
        //================================================================
        if(requestQueue == null)
        {
            requestQueue = Volley.newRequestQueue(getApplicationContext());
        }
    }

    public void makeRequest()
    {
        String url = editURL.getText().toString();
        StringRequest request = new StringRequest(Request.Method.GET,url,new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response)
            {
                println("응답->"+response);
            }
        }, new Response.ErrorListener()
        {

            @Override
            public void onErrorResponse(VolleyError error)
            {
                println("에러->" + error);
            }
        })
        {
            @Override
            protected Map<String,String> getParams() throws AuthFailureError
            {
                Map<String, String> params = new HashMap<String,String>();
                return params;
            }
        };
        request.setShouldCache(false);
        requestQueue.add(request);
        println("요청 보냄");
    }

    public void println(String data)
    {
        txtResult.append(data+"\n");
    }
}

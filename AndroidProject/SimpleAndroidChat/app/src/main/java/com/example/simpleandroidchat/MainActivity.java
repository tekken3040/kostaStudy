package com.example.simpleandroidchat;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

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
    EditText editID, editPassword;
    Button btnJoin, btnLogin;
    String mobile_ID, mobile_Password;
    String chatID, chatName;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editID = (EditText)findViewById(R.id.editID_join);
        editPassword = (EditText)findViewById(R.id.editPassword_join);
        btnJoin = (Button)findViewById(R.id.btnJoinCancel);
        btnLogin = (Button)findViewById(R.id.btnJoinOk);

        //회원가입으로 이동
        btnJoin.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent joinIntent = new Intent(MainActivity.this, join.class);
                startActivity(joinIntent);
            }
        });

        //로그인
        btnLogin.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(editID.getText().toString().equals(""))
                {
                    Toast.makeText(getApplicationContext(), "아이디를 입력해 주세요.", Toast.LENGTH_SHORT).show();
                    return;
                }
                if(editPassword.getText().toString().equals(""))
                {
                    Toast.makeText(getApplicationContext(), "비밀번호를 입력해 주세요.", Toast.LENGTH_SHORT).show();
                    return;
                }
                mobile_ID = editID.getText().toString();
                mobile_Password = editPassword.getText().toString();
                LoginThread loginThread = new LoginThread();
                loginThread.start();
            }
        });
    }

    class LoginThread extends Thread
    {
        @Override
        public void run()
        {
            super.run();
            OkHttpClient client = new OkHttpClient();
            Request.Builder builder = new Request.Builder();
            builder = builder.url("http://106.242.203.69:2740/WebAndroid/login.jsp");
            FormBody.Builder bodyBuilder = new FormBody.Builder();

            bodyBuilder.add("mobile_id", mobile_ID+"");
            bodyBuilder.add("mobile_password", mobile_Password+"");
            FormBody body = bodyBuilder.build();
            builder = builder.post(body);
            Request request = builder.build();

            LoginThread.CallbackData callbackData = new LoginThread.CallbackData();
            Call call = client.newCall(request);
            call.enqueue(callbackData);
        }

        class CallbackData implements Callback
        {
            @Override
            public void onFailure(@NotNull Call call,@NotNull IOException e)
            {
            }

            @Override
            public void onResponse(@NotNull Call call,@NotNull final Response response) throws IOException
            {
                runOnUiThread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        try
                        {
                            String result = null;
                            result = response.body().string();
                            String login = "", invalidPassword = "", invalidId = "";
                            JSONObject object = new JSONObject(result);
                            if(object.has("loginSuccess"))
                            {
                                login = object.getString("loginSuccess");
                                chatID = object.getString("id");
                                chatName = object.getString("name");
                            }
                            if(object.has("invalidPassword"))
                                invalidPassword = object.getString("invalidPassword");
                            if(object.has("invalidId"))
                                invalidId = object.getString("invalidId");

                            if(invalidId.equals("invalidId"))
                            {
                                Toast.makeText(getApplicationContext(),"존재하지 않는 아이디입니다.",Toast.LENGTH_SHORT).show();
                                return;
                            }
                            else if(invalidPassword.equals("invalidPassword"))
                            {
                                Toast.makeText(getApplicationContext(),"아이디 혹은 비밀번호가 틀렸습니다.",Toast.LENGTH_SHORT).show();
                                return;
                            }
                            else if(login.equals("loginSuccess"))
                            {
                                Toast.makeText(getApplicationContext(),"로그인에 성공하였습니다.",Toast.LENGTH_SHORT).show();
                                Intent loginIntent = new Intent(MainActivity.this, chatLobby.class);
                                loginIntent.putExtra("chatID", chatID);
                                loginIntent.putExtra("chatName", chatName);
                                startActivity(loginIntent);
                            }
                        }
                        catch(JSONException|IOException e)
                        {
                            e.printStackTrace();
                            Toast.makeText(getApplicationContext(),"에러가 발생하였습니다.",Toast.LENGTH_SHORT).show();
                        }
                    }
                });
            }
        }
    }
}

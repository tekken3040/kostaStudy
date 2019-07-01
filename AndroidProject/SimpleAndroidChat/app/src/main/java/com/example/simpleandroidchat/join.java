package com.example.simpleandroidchat;

import androidx.appcompat.app.AppCompatActivity;

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

public class join extends AppCompatActivity
{
    EditText editID_join, editName_join, editPassword_join, editPassword2_join;
    Button btnJoinOk, btnJoinCancel;
    String mobile_ID, mobile_Name, mobile_Password;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_join);
        editID_join = (EditText)findViewById(R.id.editID_join);
        editName_join = (EditText)findViewById(R.id.editName_join);
        editPassword_join = (EditText)findViewById(R.id.editPassword_join);
        editPassword2_join = (EditText)findViewById(R.id.editPassword2_join);
        btnJoinOk = (Button)findViewById(R.id.btnJoinOk);
        btnJoinCancel = (Button)findViewById(R.id.btnJoinCancel);
        //회원가입 확인
        btnJoinOk.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                String tempPass1, tempPass2;
                tempPass1 = editPassword_join.getText().toString();
                tempPass2 = editPassword2_join.getText().toString();

                if(!tempPass1.equals(tempPass2))
                {
                    Toast.makeText(getApplicationContext(), "비밀번호를 확인해 주시기 바랍니다.", Toast.LENGTH_LONG).show();
                    Toast.makeText(getApplicationContext(), editPassword_join.getText() + " : " + editPassword2_join.getText(), Toast.LENGTH_LONG).show();
                    return;
                }
                else if(editID_join.getText().toString() == null)
                {
                    Toast.makeText(getApplicationContext(), "아이디를 입력해 주세요.", Toast.LENGTH_LONG).show();
                    Toast.makeText(getApplicationContext(), editPassword_join.getText() + " : " + editPassword2_join.getText(), Toast.LENGTH_LONG).show();
                    return;
                }
                else if(editName_join.getText().toString() == null)
                {
                    Toast.makeText(getApplicationContext(), "이름을 입력해 주세요.", Toast.LENGTH_LONG).show();
                    Toast.makeText(getApplicationContext(), editPassword_join.getText() + " : " + editPassword2_join.getText(), Toast.LENGTH_LONG).show();
                    return;
                }

                mobile_ID = editID_join.getText().toString();
                mobile_Name = editName_join.getText().toString();
                mobile_Password = editPassword_join.getText().toString();
                JoinThread joinThread = new JoinThread();
                joinThread.start();
            }
        });
        //회원가입 취소
        btnJoinCancel.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                finish();
            }
        });
    }

    class JoinThread extends Thread
    {
        @Override
        public void run()
        {
            super.run();
            OkHttpClient client = new OkHttpClient();
            Request.Builder builder = new Request.Builder();
            builder = builder.url("http://106.242.203.69:2740/WebAndroid/join.jsp");
            FormBody.Builder bodyBuilder = new FormBody.Builder();

            bodyBuilder.add("mobile_id", mobile_ID+"");
            bodyBuilder.add("mobile_name", mobile_Name+"");
            bodyBuilder.add("mobile_password", mobile_Password+"");
            FormBody body = bodyBuilder.build();
            builder = builder.post(body);
            Request request = builder.build();

            CallbackData callbackData = new CallbackData();
            Call call = client.newCall(request);
            call.enqueue(callbackData);
            //bodyBuilder.add("mobile_id", mobile_id+"");
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
                            String resultTrim = null;
                            result= response.body().string();
                            resultTrim = result.trim();
                            if(!resultTrim.equals(""))
                            {
                                JSONObject object=new JSONObject(result);
                                final String name=object.getString("conflict");
                                if(name.equals("conflict"))
                                {
                                    Toast.makeText(getApplicationContext(),"아이디가 중복되었습니다.",Toast.LENGTH_SHORT).show();
                                    return;
                                }
                            }
                            else
                            {
                                Toast.makeText(getApplicationContext(),"회원가입 완료",Toast.LENGTH_SHORT).show();
                                finish();
                            }
                        }
                        catch(JSONException|IOException e)
                        {
                            e.printStackTrace();
                        }
                    }
                });
            }
        }
    }
}

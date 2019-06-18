package com.kosta.kosta04;

import android.app.Dialog;
import android.media.MediaCodec;
import android.provider.ContactsContract;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Patterns;
import android.view.KeyEvent;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.regex.Pattern;

public class EditActivity extends AppCompatActivity
{
    EditText editEmail, editPassword;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit);
        editEmail = (EditText)findViewById(R.id.editEmail);
        editEmail.setOnEditorActionListener(new TextView.OnEditorActionListener()
        {
            @Override
            public boolean onEditorAction(TextView v,int actionId,KeyEvent event)
            {
                //이메일 유효성 검사
                if(!Patterns.EMAIL_ADDRESS.matcher(editEmail.getText()).matches())
                {
                    Toast.makeText(EditActivity.this,"이메일 형식이 아닙니다",Toast.LENGTH_SHORT).show();
                    return false;
                }
                return false;
            }
        });
        editPassword = (EditText)findViewById(R.id.editPassword);
        editPassword.setOnEditorActionListener(new TextView.OnEditorActionListener()
        {
            @Override
            public boolean onEditorAction(TextView v,int actionId,KeyEvent event)
            {
                //비밀번호 유효성 검사
                if(!Pattern.matches("^(?=.*\\d)(?=.*[~`!@#$%\\^&*()-])(?=.*[a-zA-Z]).{8,20}$", editPassword.getText()))
                {
                    Toast.makeText(EditActivity.this,"비밀번호 형식을 지켜주세요.",Toast.LENGTH_SHORT).show();
                    return false;
                }
                return false;
            }
        });
    }
}

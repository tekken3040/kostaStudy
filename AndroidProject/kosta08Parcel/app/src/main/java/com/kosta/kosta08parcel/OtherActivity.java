package com.kosta.kosta08parcel;

import androidx.appcompat.app.AppCompatActivity;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class OtherActivity extends AppCompatActivity
{
    TextView txtMessage;
    EditText editInputResult;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_other);
        txtMessage = (TextView)findViewById(R.id.txtMessage);
        editInputResult = (EditText)findViewById(R.id.editInputResult);
        final Intent intent = getIntent();
        MyData data = (MyData)intent.getParcelableExtra("MYDATA");
        txtMessage.setText(data.keyword + ", age: " + data.age);
        Button btnFinish = (Button)findViewById(R.id.btnFinish);
        btnFinish.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                String result = editInputResult.getText().toString();
                Intent innerIntent = new Intent();
                innerIntent.putExtra("MESSAGE", result);
                setResult(Activity.RESULT_OK, innerIntent);
                finish();
            }
        });
    }
}

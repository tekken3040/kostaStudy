package com.kosta.kosta08parcel;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

public class MainActivity extends AppCompatActivity
{
    EditText editInput;
    TextView txtResult;
    //private static
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editInput = (EditText)findViewById(R.id.editInput);
        txtResult = (TextView)findViewById(R.id.txtResult);
        Button btnOther = (Button)findViewById(R.id.btnOther);
        btnOther.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent intent = new Intent(MainActivity.this, OtherActivity.class);
                MyData data = new MyData();
                data.keyword = "Hello";
                data.age = 33;
                intent.putExtra("MYDATA", data);
                startActivity(intent, RC_OTHER);
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode,int resultCode,@Nullable Intent data)
    {
        super.onActivityResult(requestCode,resultCode,data);
        if(requestCode == RC_OTHER)
        {
            if(resultCode ==Activity.RESULT_OK)
            {
                String text = data.getStringExtra("MESSAGE");
                txtResult.setText(text);
            }
            else
            {
                Toast.makeText(this, "Result Canceled", Toast.LENGTH_SHORT).show();
            }
        }
    }
}

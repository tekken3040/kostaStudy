package com.kosta.kosta04;

import android.graphics.Color;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

public class TextViewActivity extends AppCompatActivity
{
    Button btnChange, btnSelection;
    TextView txtMessage;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_text_view);
        txtMessage = (TextView)findViewById(R.id.txtMessage);

        btnSelection = (Button)findViewById(R.id.btnSelection);
        btnSelection.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                if(txtMessage.isSelected())
                {
                    txtMessage.setTextColor(Color.RED);
                    txtMessage.setSelected(false);
                }
                else
                {
                    txtMessage.setTextColor(Color.BLUE);
                    txtMessage.setSelected(true);
                }
                //txtMessage.setSelected(!txtMessage.isSelected());
            }
        });

        btnChange = (Button)findViewById(R.id.btnChange);
        btnChange.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                txtMessage.setText("change txt message");
            }
        });
    }
}

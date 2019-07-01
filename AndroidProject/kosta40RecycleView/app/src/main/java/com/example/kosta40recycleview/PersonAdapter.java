package com.example.kosta40recycleview;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;

public class PersonAdapter extends RecyclerView.Adapter<PersonAdapter.ViewHolder>
{
    ArrayList<Person> items = new ArrayList<Person>();
    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent,int viewType)
    {
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());
        View itemView = inflater.inflate(R.layout.person_item, parent, false);
        return new ViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder,int position)
    {
        Person item = items.get(position);
        holder.setItem(item);
    }

    public void addItem(Person item)
    {
        items.add(item);
    }

    @Override
    public int getItemCount()
    {
        return items.size();
    }

    public Person getItem(int position)
    {
        return items.get(position);
    }

    public void setItem(int position, Person item)
    {
        items.set(position, item);
    }

    public void setItems(ArrayList<Person> items)
    {
        this.items=items;
    }

    static class ViewHolder extends RecyclerView.ViewHolder
    {
        TextView txtName, txtMobile;

        public ViewHolder(@NonNull View itemView)
        {
            super(itemView);
            txtName = itemView.findViewById(R.id.txtName);
            txtMobile = itemView.findViewById(R.id.txtMobile);
        }
        public void setItem(Person item)
        {
            txtName.setText(item.getName());
            txtMobile.setText(item.getMobile());
        }
    }
}

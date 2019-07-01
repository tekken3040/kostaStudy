package com.example.kosta41recyclegrid;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;

public class ProductAdapter extends RecyclerView.Adapter<ProductAdapter.ViewHolder> implements OnProductItemClickListener
{
    ArrayList<Product> items = new ArrayList<Product>();
    OnProductItemClickListener onProductItemClickListener;
    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent,int viewType)
    {
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());
        View itemView = inflater.inflate(R.layout.product_item, parent, false);

        return new ViewHolder(itemView, this);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder,int position)
    {
        Product item = items.get(position);
        holder.setItem(item);
    }

    @Override
    public int getItemCount()
    {
        return items.size();
    }

    public void addItem(Product item)
    {
        items.add(item);
    }

    public void setItem(int position, Product item)
    {
        this.items.set(position, item);
    }

    public Product getItem(int position)
    {
        return items.get(position);
    }

    public void setItems(ArrayList<Product> items)
    {
        this.items=items;
    }

    public ArrayList<Product> getItems()
    {
        return items;
    }

    public void setOnProductItemClickListener(OnProductItemClickListener listener)
    {
        this.onProductItemClickListener = listener;
    }
    @Override
    public void onItemClick(ViewHolder holder,View view,int position)
    {
        if(onProductItemClickListener != null)
        {
            onProductItemClickListener.onItemClick(holder, view, position);
        }
    }

    static class ViewHolder extends RecyclerView.ViewHolder
    {
        TextView txtCountAgent, txtName, txtPrice;
        ImageView imageView;

        public ViewHolder(@NonNull View itemView, final OnProductItemClickListener listener)
        {
            super(itemView);
            txtCountAgent = itemView.findViewById(R.id.txtCountAgent);
            txtName = itemView.findViewById(R.id.txtName);
            txtPrice = itemView.findViewById(R.id.txtPrice);
            imageView = itemView.findViewById(R.id.imageView);
            itemView.setOnClickListener(new View.OnClickListener()
            {
                @Override
                public void onClick(View view)
                {
                    int position = getAdapterPosition();
                    if(listener != null)
                        listener.onItemClick(ViewHolder.this, view, position);
                }
            });
        }

        public void setItem(Product item)
        {
            txtCountAgent.setText(String.valueOf(item.getCountAgent()));
            txtName.setText(item.getName());
            txtPrice.setText(String.valueOf(item.getPrice()));
            imageView.setImageResource(item.getImageRes());
        }
    }
}

import { ProductResumeViewModel } from '@/app/models/product-resume-view-model';
import { OrderResumeViewModel } from '../models/order-resume-view-model';

export async function fetchProducts(): Promise<ProductResumeViewModel[]> {
  try {
    const response = await fetch('https://localhost:61868/Products');
    
    if (!response.ok) {
      throw new Error('Erro ao buscar produtos');
    }

    const data = await response.json();

    const products: ProductResumeViewModel[] = data.map((product: any) => ({
      id: product.id,
      name: product.name,
      quantityInStock: product.quantityInStock,
    }));

    return products;
  } catch (error) {
    console.error('Erro ao buscar produtos:', error);
    return [];  
  }
}

export async function fetchOrders(): Promise<OrderResumeViewModel[]> {
  try {
    const response = await fetch('https://localhost:61868/Orders');
    
    if (!response.ok) {
      throw new Error('Erro ao buscar encomendas');
    }

    const data = await response.json();
    const orders: OrderResumeViewModel[] = data.map((order: any) => ({
      id: order.id,
      status: order.status,
      quantity: order.quantity,
      productName: order.productName,
    }));

    return orders;
  } catch (error) {
    console.error('Erro ao buscar encomendas:', error);
    return [];  
  }
}
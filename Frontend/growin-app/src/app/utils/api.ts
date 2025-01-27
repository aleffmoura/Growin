import { ProductResumeViewModel } from '@/app/models/product-resume-view-model';
import { OrderResumeViewModel } from '../models/order-resume-view-model';

export async function fetchProducts(): Promise<ProductResumeViewModel[]> {
  try {
    const response = await fetch('https://localhost:7113/Products');
    
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
    const response = await fetch('https://localhost:7113/Orders');
    
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
export async function patchOrder(orderId: number): Promise<string> {
  try {
    const response = await fetch(`https://localhost:7113/Orders/${orderId}`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
      }
    });

    if (response.ok) {
      return '';
    }
    console.log(response);
    
    const errorData = await response.json();
    
    return `Erro ${errorData.title} - ${errorData.detail}`;
  } catch (error) {
    return `Erro ao comunicar com a API: ${error}`;
  }
}

export async function createOrder(productId: number, quantity: number): Promise<ApiResponse> {
  try {
    console.log(productId,quantity )
    const response = await fetch('https://localhost:7113/Orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        productId,
        quantity,
      }),
    });

    if (response.ok) {
      return { success: true };
    }

    if (response.status === 400) {
      const errorResponse = await response.json();
      const errorDetails = JSON.parse(errorResponse.detail);
      if (Array.isArray(errorDetails)) {
        const formattedErrors = errorDetails.map((err: any) => ({
          propertyName: err.PropertyName,
          errorMessage: err.ErrorMessage,
        }));
        return { success: false, errors: formattedErrors };
      }

      console.error('Formato inesperado em detail:', errorResponse.detail);
      return { success: false };
    }

    console.error('Erro inesperado ao criar a ordem.');
    return { success: false };
  } catch (error) {
    console.error('Erro de conexão ao criar encomenda:', error);
    return { success: false };
  }
}

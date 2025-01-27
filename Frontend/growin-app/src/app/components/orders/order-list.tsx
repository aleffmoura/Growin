import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Button from '@mui/material/Button';
import { fetchOrders, patchOrder } from '@/app/utils/api';  // Importando patchOrder
import { Alert, Box, CircularProgress } from '@mui/material';
import { useEffect, useState } from 'react';
import { OrderResumeViewModel } from '@/app/models/order-resume-view-model';

export default function OrdersComponent() {
  const [orders, setOrders] = useState<OrderResumeViewModel[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [updatingOrders, setUpdatingOrders] = useState<{ [key: number]: boolean }>({});
  
  useEffect(() => {
    async function loadOrders() {
      const response = await fetchOrders();
      setOrders(response);
      setLoading(false);
    }

    loadOrders();
  }, []);

  if (loading) return (<Box sx={{ display: 'flex' }}><CircularProgress /></Box>);

  const handleFinalize = async (orderId: number) => {
    setUpdatingOrders((prev) => ({ ...prev, [orderId]: true }));

    const error = await patchOrder(orderId);

    if (!error) {
      setOrders(prevOrders =>
        prevOrders.map(order =>
          order.id === orderId ? { ...order, status: 'Completed' } : order
        )
      );
    }else{
      setError(error);
    }

    setUpdatingOrders((prev) => ({ ...prev, [orderId]: false }));
  };

  return (
    <>
      {error ? <Alert severity="error">{error}</Alert> : ''}
      
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 550 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Id</TableCell>
              <TableCell align="right">Nome do produto</TableCell>
              <TableCell align="right">Quantidade da encomenda</TableCell>
              <TableCell align="right">Status</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orders.map((row) => (
              <TableRow key={row.id} sx={{ '&:last-child td, &:last-child th': { border: 0 } }} >
                <TableCell component="th" scope="row">
                  {row.id}
                </TableCell>
                <TableCell align="right">{row.productName}</TableCell>
                <TableCell align="right">{row.quantity}</TableCell>
                <TableCell align="right">{row.status}</TableCell>
                <TableCell align="right">
                  <Button disabled={row.status.toLowerCase() !== 'reserved' || updatingOrders[row.id]} onClick={() => handleFinalize(row.id)} >
                    {updatingOrders[row.id] ? 'Finalizando...' : 'Finalizar'}
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
}

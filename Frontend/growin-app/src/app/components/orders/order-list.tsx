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
import { fetchOrders } from '@/app/utils/api';
import { Box, CircularProgress } from '@mui/material';
import { useEffect, useState } from 'react';
import { OrderResumeViewModel } from '@/app/models/order-resume-view-model';

export default function OrdersComponent() {
  const [orders, setOrders] = useState<OrderResumeViewModel[]>([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    async function loadOrders() {
      const response = await fetchOrders();
      setOrders(response);
      setLoading(false);
    }
  
    loadOrders();
  }, []);
  
  if (loading) return (<Box sx={{ display: 'flex' }}><CircularProgress /></Box>);
  
  return (
    <>
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
                <TableCell align="right"><Button disabled={row.status.toLowerCase() !== 'reserved'}>Finalizar</Button></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
}

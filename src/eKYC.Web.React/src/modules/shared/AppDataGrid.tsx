import { DataGrid, DataGridProps, GridValidRowModel } from '@mui/x-data-grid';
import { SxProps, Theme } from '@mui/material';

type Props<R extends GridValidRowModel> = Omit<DataGridProps<R>, 'sx'> & { sx?: SxProps<Theme> };

/** DataGrid with the project defaults: compact rows, auto height, 25 rows per page, no checkbox selection. */
export default function AppDataGrid<R extends GridValidRowModel>({ sx, ...props }: Props<R>) {
  return (
    <DataGrid<R>
      density="compact"
      autoHeight
      disableRowSelectionOnClick
      pageSizeOptions={[10, 25, 50, 100]}
      initialState={{ pagination: { paginationModel: { pageSize: 25, page: 0 } } }}
      sx={{ backgroundColor: 'background.paper', ...sx }}
      {...props}
    />
  );
}

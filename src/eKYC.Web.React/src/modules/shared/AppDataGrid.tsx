import { SxProps, Theme } from '@mui/material';
import { DataGrid, DataGridProps, GridValidRowModel } from '@mui/x-data-grid';
import EmptyState from './EmptyState';

type Props<R extends GridValidRowModel> = Omit<DataGridProps<R>, 'sx'> & {
  sx?: SxProps<Theme>;
  /** Text for the empty overlay (after filtering/searching inside the grid). */
  emptyTitle?: string;
};

/**
 * DataGrid with the project defaults: compact rows, auto height, toolbar (columns / filters / density / export / search),
 * 25 rows per page, a friendly empty overlay and no checkbox selection.
 */
export default function AppDataGrid<R extends GridValidRowModel>({ sx, emptyTitle = 'Nema rezultata', ...props }: Props<R>) {
  return (
    <DataGrid<R>
      density="compact"
      autoHeight
      showToolbar
      disableRowSelectionOnClick
      pageSizeOptions={[10, 25, 50, 100]}
      initialState={{ pagination: { paginationModel: { pageSize: 25, page: 0 } } }}
      slots={{ noRowsOverlay: () => <EmptyState title={emptyTitle} description="Promijenite pretragu ili filtre." /> }}
      sx={[{ backgroundColor: 'background.paper' }, ...(Array.isArray(sx) ? sx : [sx])]}
      {...props}
    />
  );
}

/** sx fragment: pointer cursor + highlighted selected row, for grids where a row click opens details or selects. */
export const clickableRowsSx = {
  '& .MuiDataGrid-row': { cursor: 'pointer' },
  '& .selected-row, & .selected-row:hover': { backgroundColor: 'action.selected' },
};

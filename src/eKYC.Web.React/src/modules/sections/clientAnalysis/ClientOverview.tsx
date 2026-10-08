import ClientAnalysisBase from './ClientAnalysisBase';

/** "Klijenti - pregled" tab - the read-only view of the same data as "Klijenti - analiza". */
export default function ClientOverview() {
  return (
    <ClientAnalysisBase
      title="Klijenti - pregled"
      subtitle="Pregled klijenata odabranog tipa, samo za čitanje — za obradu klijenata vidi Klijenti - analiza."
    />
  );
}

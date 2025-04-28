import Game from "../components/Game";
import Pose from "../components/Pose";

export default async function HomePage() {
  const data = await getData();

  return (
    <div>
      <title>{data.title}</title>
      <Game />
      <Pose />
    </div>
  );
}

const getData = async () => {
  const data = {
    title: 'Ninja Paws',
  };

  return data;
};

export const getConfig = async () => {
  return {
    render: 'static',
  } as const;
};

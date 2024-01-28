import * as Schema from '../queries/api/squidjamSchemas';

export default function Creature({
    creature,
    creatureIndex,
    player,
}: {
    creature: Schema.Creature;
    creatureIndex: number;
    player: Schema.Player;
}) {
    return <div>{JSON.stringify(creature, undefined, 2)}</div>;
}

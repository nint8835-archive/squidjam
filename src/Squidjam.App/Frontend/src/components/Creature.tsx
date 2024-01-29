import * as Schema from '../queries/api/squidjamSchemas';
import { cn } from '../util';

const classClasses: Record<Exclude<Schema.Player['class'], null>['type'], string> = {
    // TODO: Different font for Grack?
    Grack: 'heropattern-floatingcogs-zinc-950 font-mono',
    Gump: 'heropattern-eyes-zinc-950 font-gump',
};

export default function Creature({
    creature,
    creatureIndex,
    player,
}: {
    creature: Schema.Creature;
    creatureIndex: number;
    player: Schema.Player;
}) {
    return (
        <div
            className={cn(
                'flex w-48 min-w-48 flex-col items-center rounded-md border-[1px] border-black bg-black bg-opacity-30',
                classClasses[player.class!.type],
            )}
        >
            <div className="flex w-full justify-between border-b-[1px] border-black bg-black bg-opacity-35 px-2">
                <div className="flex w-6 flex-row">{creature.health}</div>
                <div className="flex flex-1 justify-center">{creature.name}</div>
                <div className="flex w-6 flex-row-reverse">{creature.attack}</div>
            </div>
            <p className="p-1">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam eleifend dolor ut porttitor lacinia.
                Quisque vel blandit odio. Maecenas sed purus at purus fermentum scelerisque in non lorem. Vestibulum
                euismod aliquam neque a luctus. Nunc.
            </p>
        </div>
    );
}
